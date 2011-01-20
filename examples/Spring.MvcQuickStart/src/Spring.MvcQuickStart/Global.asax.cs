﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using Spring.Web.Mvc;
using Spring.Context.Support;

namespace Spring.MvcQuickStart
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : SpringMvcApplication
    {
        protected override void ConfigureApplicationContext()
        {
            CodeConfigApplicationContext codeConfigApplicationContext = new CodeConfigApplicationContext();
            codeConfigApplicationContext.ScanAllAssemblies();
            codeConfigApplicationContext.Refresh();
            ContextRegistry.RegisterContext(codeConfigApplicationContext);
        }
    }
}