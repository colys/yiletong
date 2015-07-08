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
    internal unsafe struct cef_display_handler_t
    {
        internal cef_base_t _base;
        internal IntPtr _on_address_change;
        internal IntPtr _on_title_change;
        internal IntPtr _on_tooltip;
        internal IntPtr _on_status_message;
        internal IntPtr _on_console_message;
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int add_ref_delegate(cef_display_handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int release_delegate(cef_display_handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int get_refct_delegate(cef_display_handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate void on_address_change_delegate(cef_display_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_string_t* url);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate void on_title_change_delegate(cef_display_handler_t* self, cef_browser_t* browser, cef_string_t* title);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int on_tooltip_delegate(cef_display_handler_t* self, cef_browser_t* browser, cef_string_t* text);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate void on_status_message_delegate(cef_display_handler_t* self, cef_browser_t* browser, cef_string_t* value);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int on_console_message_delegate(cef_display_handler_t* self, cef_browser_t* browser, cef_string_t* message, cef_string_t* source, int line);
        
        private static int _sizeof;
        
        static cef_display_handler_t()
        {
            _sizeof = Marshal.SizeOf(typeof(cef_display_handler_t));
        }
        
        internal static cef_display_handler_t* Alloc()
        {
            var ptr = (cef_display_handler_t*)Marshal.AllocHGlobal(_sizeof);
            *ptr = new cef_display_handler_t();
            ptr->_base._size = (UIntPtr)_sizeof;
            return ptr;
        }
        
        internal static void Free(cef_display_handler_t* ptr)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
        
    }
}
