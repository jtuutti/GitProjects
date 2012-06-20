using System.Reflection;
using System.Runtime.InteropServices;
using RestTest.App_Start;

[assembly: AssemblyTitle("RestTest")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("RestTest")]
[assembly: AssemblyCopyright("Copyright ©  2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: Guid("04438a4b-579f-4ba8-9d63-312b3dae5331")]

[assembly: WebActivator.PreApplicationStartMethod(typeof(ServiceBootstrapper), "RegisterDependencies")]
