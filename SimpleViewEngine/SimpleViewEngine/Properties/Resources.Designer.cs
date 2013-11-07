﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SimpleViewEngine.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SimpleViewEngine.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Controller type &apos;{0}&apos; could not be created or does not subclass the IController interface.
        /// </summary>
        internal static string InvalidControllerType {
            get {
                return ResourceManager.GetString("InvalidControllerType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Layout &apos;{0}&apos; does not have the &apos;.layout.html&apos; file extension.
        /// </summary>
        internal static string InvalidLayoutViewExtension {
            get {
                return ResourceManager.GetString("InvalidLayoutViewExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Partial &apos;{0}&apos; does not have the &apos;.partial.html&apos; file extension.
        /// </summary>
        internal static string InvalidPartialViewExtension {
            get {
                return ResourceManager.GetString("InvalidPartialViewExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to View &apos;{0}&apos; does not have the &apos;.html&apos; file extension.
        /// </summary>
        internal static string InvalidViewExtension {
            get {
                return ResourceManager.GetString("InvalidViewExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Layout &apos;{0}&apos; was not found.
        /// </summary>
        internal static string LayoutViewNotFound {
            get {
                return ResourceManager.GetString("LayoutViewNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Partial view &apos;{0}&apos; was not found.
        /// </summary>
        internal static string PartialViewNotFound {
            get {
                return ResourceManager.GetString("PartialViewNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Partial view &apos;{0}&apos; is referenced recursively.
        /// </summary>
        internal static string RecursivePartialViewReference {
            get {
                return ResourceManager.GetString("RecursivePartialViewReference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ASP .NET server cache is disabled or not available. Instantiate the HtmlViewCache without the HTML cache by passing false in the constructor..
        /// </summary>
        internal static string UnavailableAspCache {
            get {
                return ResourceManager.GetString("UnavailableAspCache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Layouts cannot be referenced by name. Use the &apos;url&apos; attribute instead and specify a relative path to the layout file..
        /// </summary>
        internal static string UnsupportedLayoutName {
            get {
                return ResourceManager.GetString("UnsupportedLayoutName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to View &apos;{0}&apos; was not found.
        /// </summary>
        internal static string ViewNotFound {
            get {
                return ResourceManager.GetString("ViewNotFound", resourceCulture);
            }
        }
    }
}
