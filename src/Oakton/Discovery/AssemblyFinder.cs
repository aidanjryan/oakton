using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if NETSTANDARD2_0

#endif

namespace Oakton.Discovery
{
	public static class AssemblyFinder
    {
        public static IEnumerable<Assembly> FindAssemblies(Action<string> logFailure, bool includeExeFiles)
        {
            string path;
            try {
                path = AppContext.BaseDirectory;
            }
            catch (Exception) {
                path = System.IO.Directory.GetCurrentDirectory();
            }

            return FindAssemblies(path, logFailure, includeExeFiles);
        }

        public static IEnumerable<Assembly> FindAssemblies(string assemblyPath, Action<string> logFailure, bool includeExeFiles)
        {
            var dllFiles = Directory.EnumerateFiles(assemblyPath, "*.dll", SearchOption.AllDirectories);
            var files = dllFiles;

            if(includeExeFiles)
            {
                var exeFiles = Directory.EnumerateFiles(assemblyPath, "*.exe", SearchOption.AllDirectories);
                files = dllFiles.Concat(exeFiles);
            }

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Assembly assembly = null;

                try
                {
#if NETSTANDARD2_0
                    assembly = OaktonAssemblyContext.Loader.LoadFromAssemblyName(new AssemblyName(name));
#else
                    assembly = AppDomain.CurrentDomain.Load(new AssemblyName(name));
#endif
                }
                catch (Exception)
                {
                    try
                    {
#if NETSTANDARD2_0
                        assembly = OaktonAssemblyContext.Loader.LoadFromAssemblyPath(file);
#else
                        assembly = Assembly.LoadFrom(file);
#endif
                    }
                    catch (Exception)
                    {
                        logFailure(file);
                    }
                }

                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }




        internal static IEnumerable<Assembly> FindAssemblies(Func<Assembly, bool> filter,
            Action<string> onDirectoryFound = null, bool includeExeFiles=false)
        {
            if (filter == null)
            {
                filter = a => true;
            }

            if (onDirectoryFound == null)
            {
                onDirectoryFound = dir => { };
            }

            return FindAssemblies(file => { }, includeExeFiles: includeExeFiles).Where(filter);
        }
    }
}
