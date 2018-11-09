﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SetupAssistent.Model;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace SetupAssistent.ViewModel
{
    public class CreateNewModuleViewModel
    {
        #region Public Propertys
        public Module NewModule { get; set; }
        public MyICommand CancelCommand { get; set; }
        public MyICommand SaveCommand { get; set; }
        public MyICommand PicturePickerCommand { get; set; }
        private readonly NavigationViewModel _navigationViewModel;
        public string userName = Environment.UserName.ToString();
        public string ModuleOutputFile = string.Empty;
        private readonly Module ExistingModule;
        private readonly int ModuleIndex;
        #endregion

        #region Default Constructor
        public CreateNewModuleViewModel(NavigationViewModel navigationViewModel, Module existingModule, int index)
        {
            ExistingModule = existingModule;
            ModuleIndex = index;
            _navigationViewModel = navigationViewModel;
            CancelCommand = new MyICommand(onCancelCommand, canCancelCommand);
            SaveCommand = new MyICommand(onSaveCommand, canSaveCommand);
            PicturePickerCommand = new MyICommand(onPicturePickerCommand, canPicturePickerCommand);
            //output path will be set in settings later... I should propbably work on that.
            ModuleOutputFile = AllSettings.settings[0].OutputFilePath + "\\Modules.xml";
            initNewModule();
        }
        #endregion

        #region InitNewModule
        public void initNewModule()
        {
            Module tempModule = new Module();

            if(ExistingModule != null)
            {
                //I have some potential bugs here. Still looking into it.
                tempModule.Name = ExistingModule.Name.ToString();
                tempModule.Description = ExistingModule.Description.ToString();
                tempModule.DescriptionPreview = ExistingModule.DescriptionPreview.ToString();
                tempModule.ImageSource = ExistingModule.ImageSource.ToString();
                tempModule.TasksList = ExistingModule.TasksList;
            }

            NewModule = tempModule;
        }
        #endregion

        #region Commands Code
        public void onCancelCommand(object parameter)
        {
            //maybe add a confirm popup.
            _navigationViewModel.SelectedViewModel = new ModuleViewModel(_navigationViewModel);
        }
        public bool canCancelCommand()
        {
            return true;
        }

        public void onSaveCommand(object parameter)
        {
            //save data in xml format using xmlserializer.
            if(NewModule.Name != "")
            {
                //Check settings for ModuleNameSize limit or something here.
                if (AllSettings.settings[0].LimitModuleNameSize ? NewModule.Name.Length > 20 : false)
                {
                    MessageBox.Show(String.Format("The name '{0}' is too long. Please shorten the name or disable this feature in settings.", NewModule.Name.ToString()), "Name is too long", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    bool nameUsed = false;
                    foreach (Module module in AllModules.modulesList)
                    {
                        if (ExistingModule != null)
                        {

                            if (ExistingModule.Name == NewModule.Name ? false : module.Name == NewModule.Name)
                            {
                                nameUsed = true;
                            }
                        }
                        else
                        {
                            if(module.Name == NewModule.Name)
                            {
                                nameUsed = true;
                            }
                        }
                    }

                    if (!nameUsed)
                    {
                        try
                        {
                            if(ExistingModule != null)
                            {
                                if (AllModules.modulesList != null)
                                {
                                    AllModules.modulesList.RemoveAt(ModuleIndex);
                                }
                            }
                            bool saved = false;

                            using (TextWriter writer = new StreamWriter(ModuleOutputFile))
                            {
                                ObservableCollection<Module> tempOC = new ObservableCollection<Module>();
                                tempOC = AllModules.modulesList;
                                tempOC.Add(NewModule);

                                XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<Module>));
                                xmlS.Serialize(writer, tempOC);

                                saved = true;
                            }

                            if (saved)
                            {
                                MessageBox.Show(String.Format("Module '{0}' was saved.", NewModule.Name.ToString()), "Module Saved");  // This will be replaced with a delayed animation in the future. For now this works well enough for confirmation.  <<<------ IMPORTANT
                                _navigationViewModel.SelectedViewModel = new ModuleViewModel(_navigationViewModel);
                            }
                            else
                            {
                                MessageBox.Show(String.Format("Something went wroung.{0}Could not save Module.", Environment.NewLine), "Oops  :(", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show(String.Format("The name '{0}' is currently in use by another module.{1}Please choose another name.", NewModule.Name.ToString(), Environment.NewLine), "Name Already In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show(String.Format("New Modules must have a name.{0}{0}Please enter a name for this module.",Environment.NewLine), "Module Not Named", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public bool canSaveCommand()
        {
            return true;
        }

        public void onPicturePickerCommand(object parameter)
        {
            Microsoft.Win32.OpenFileDialog filePicker = new Microsoft.Win32.OpenFileDialog();
            filePicker.Filter = "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|"
                              + "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";
            filePicker.FilterIndex = 6;

            Nullable<bool> result = filePicker.ShowDialog();
            if(result == true)
            {
                NewModule.ImageSource = filePicker.FileName.ToString();
            }
        }
        public bool canPicturePickerCommand()
        {
            return true;
        }
#endregion
    }
}
