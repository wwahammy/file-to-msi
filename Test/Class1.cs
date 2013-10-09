using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileToMsi;
using Microsoft.Deployment.WindowsInstaller;
using Xunit;

namespace Test
{
    public class Class1
    {
        [Fact]
        public void Test1()
        {
            var o = new Program();
            int[] hash = new int[4];
            Installer.GetFileHash(@"C:\Users\Eric\Desktop\NuPatternToolkitBuilderAndLabs-Unsigned.msi", hash);
            o.SearchMsiFileHashsForOurHash(@"C:\Users\Eric\Desktop\OutercurveCmdlets.msi", hash);
        }
    }
}
