﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb;

namespace Chai.WorkflowManagment.Modules.Shell.Views
{
    public class UserInfoPresenter : Presenter<IUserInfoView>
    {
        private ShellController _controller;

        public UserInfoPresenter()
        {
        }

        public override void OnViewLoaded()
        {
            View.user = Controller.GetCurrentUser();
        }

        public override void OnViewInitialized()
        {
            // TODO: Implement code that will be executed the first time the view loads
        }

        [CreateNew]
        public ShellController Controller
        {
            get
            {
                return _controller;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this._controller = value;
            }
        }
    }
}




