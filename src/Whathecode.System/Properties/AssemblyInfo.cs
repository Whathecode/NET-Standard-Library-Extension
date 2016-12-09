using System.Reflection;
using System.Runtime.InteropServices;


[assembly: AssemblyProduct("Whathecode.System")]
[assembly: AssemblyCompany("Whathecode")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1c3067d6-a2ed-45dc-93b2-b7dbe0762e31")]
