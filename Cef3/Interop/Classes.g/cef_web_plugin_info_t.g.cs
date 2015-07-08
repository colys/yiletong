//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
namespace Cef3.Interop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Security;
    
    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
    internal unsafe struct cef_web_plugin_info_t
    {
        internal cef_base_t _base;
        internal IntPtr _get_name;
        internal IntPtr _get_path;
        internal IntPtr _get_version;
        internal IntPtr _get_description;
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int add_ref_delegate(cef_web_plugin_info_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int release_delegate(cef_web_plugin_info_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int get_refct_delegate(cef_web_plugin_info_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_name_delegate(cef_web_plugin_info_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_path_delegate(cef_web_plugin_info_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_version_delegate(cef_web_plugin_info_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_description_delegate(cef_web_plugin_info_t* self);
        
        // AddRef
        private static IntPtr _p0;
        private static add_ref_delegate _d0;
        
        public static int add_ref(cef_web_plugin_info_t* self)
        {
            add_ref_delegate d;
            var p = self->_base._add_ref;
            if (p == _p0) { d = _d0; }
            else
            {
                d = (add_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(add_ref_delegate));
                if (_p0 == IntPtr.Zero) { _d0 = d; _p0 = p; }
            }
            return d(self);
        }
        
        // Release
        private static IntPtr _p1;
        private static release_delegate _d1;
        
        public static int release(cef_web_plugin_info_t* self)
        {
            release_delegate d;
            var p = self->_base._release;
            if (p == _p1) { d = _d1; }
            else
            {
                d = (release_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(release_delegate));
                if (_p1 == IntPtr.Zero) { _d1 = d; _p1 = p; }
            }
            return d(self);
        }
        
        // GetRefCt
        private static IntPtr _p2;
        private static get_refct_delegate _d2;
        
        public static int get_refct(cef_web_plugin_info_t* self)
        {
            get_refct_delegate d;
            var p = self->_base._get_refct;
            if (p == _p2) { d = _d2; }
            else
            {
                d = (get_refct_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_refct_delegate));
                if (_p2 == IntPtr.Zero) { _d2 = d; _p2 = p; }
            }
            return d(self);
        }
        
        // GetName
        private static IntPtr _p3;
        private static get_name_delegate _d3;
        
        public static cef_string_userfree* get_name(cef_web_plugin_info_t* self)
        {
            get_name_delegate d;
            var p = self->_get_name;
            if (p == _p3) { d = _d3; }
            else
            {
                d = (get_name_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_name_delegate));
                if (_p3 == IntPtr.Zero) { _d3 = d; _p3 = p; }
            }
            return d(self);
        }
        
        // GetPath
        private static IntPtr _p4;
        private static get_path_delegate _d4;
        
        public static cef_string_userfree* get_path(cef_web_plugin_info_t* self)
        {
            get_path_delegate d;
            var p = self->_get_path;
            if (p == _p4) { d = _d4; }
            else
            {
                d = (get_path_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_path_delegate));
                if (_p4 == IntPtr.Zero) { _d4 = d; _p4 = p; }
            }
            return d(self);
        }
        
        // GetVersion
        private static IntPtr _p5;
        private static get_version_delegate _d5;
        
        public static cef_string_userfree* get_version(cef_web_plugin_info_t* self)
        {
            get_version_delegate d;
            var p = self->_get_version;
            if (p == _p5) { d = _d5; }
            else
            {
                d = (get_version_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_version_delegate));
                if (_p5 == IntPtr.Zero) { _d5 = d; _p5 = p; }
            }
            return d(self);
        }
        
        // GetDescription
        private static IntPtr _p6;
        private static get_description_delegate _d6;
        
        public static cef_string_userfree* get_description(cef_web_plugin_info_t* self)
        {
            get_description_delegate d;
            var p = self->_get_description;
            if (p == _p6) { d = _d6; }
            else
            {
                d = (get_description_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_description_delegate));
                if (_p6 == IntPtr.Zero) { _d6 = d; _p6 = p; }
            }
            return d(self);
        }
        
    }
}
