using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Shared;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public class TelephoneExtensionPresenter : Presenter<ITelephoneExtensionView>
    {

        // NOTE: Uncomment the following code if you want ObjectBuilder to inject the module controller
        //       The code will not work in the Shell module, as a module controller is not created by default
        //
        private Chai.WorkflowManagment.Modules.Setting.SettingController _controller;
        public TelephoneExtensionPresenter([CreateNew] Chai.WorkflowManagment.Modules.Setting.SettingController controller)
        {
            _controller = controller;
        }

        public override void OnViewLoaded()
        {
            View.telephoneextension = _controller.ListTelephoneExtensions(View.Name,View.Extension);
        }

        public override void OnViewInitialized()
        {
            
        }
        public IList<TelephoneExtension> GetTelephoneExtensions()
        {
            return _controller.GetTelephoneExtensions();
        }

        public void SaveOrUpdateTelephoneExtension(TelephoneExtension telext)
        {
            _controller.SaveOrUpdateEntity(telext);
        }

        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Setting/Default.aspx?{0}=3", AppConstants.TABID));
        }

        public void DeleteTelephoneExtension(TelephoneExtension TelephoneExtension)
        {
            _controller.DeleteEntity(TelephoneExtension);
        }
        public TelephoneExtension GetTelephoneExtensionById(int id)
        {
            return _controller.GetTelephoneExtension(id);
        }

        public IList<TelephoneExtension> ListTelephoneExtensions(string Name,string Extension)
        {
            return _controller.ListTelephoneExtensions(Name, Extension);
          
        }
        public void Commit()
        {
            _controller.Commit();
        }
    }
}




