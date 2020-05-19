using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JuliHelper.Content
{
    public static class DynamicContent
    {
        static string outputDir = Path.Combine(G.exeDir, "Content","dynamic");
        static string mgcbPath = Path.Combine(G.exeDir, "MGCB","MGCB.exe");// @"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\MGCB.exe";
        static string intermediateDir = Path.Combine(G.exeDir, "temp");
        static string effectCopyPath = Path.Combine(outputDir, "__effect.xnb");

        public static void Initialize(string processedEffectPath)
        {
            //clear outputDir
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
                Thread.Sleep(10);
            }
            while (Directory.Exists(outputDir))
                Thread.Sleep(10);

            Directory.CreateDirectory(outputDir);

            do
            {
                Thread.Sleep(10);
            } while (!Directory.Exists(outputDir));

            //copy effect to outputDir (root)
            File.Copy(processedEffectPath, effectCopyPath);

            if (!Directory.Exists(intermediateDir))
                Directory.CreateDirectory(intermediateDir);
        }

        public static void ClearDynamicContentFolder()
        {
            //clear everything in \\dynamic\\ but __effect.xnb
            Directory.GetFiles(outputDir, "*", SearchOption.TopDirectoryOnly).Where(f => f != effectCopyPath).ToList().ForEach(f => File.Delete(f));
            Directory.GetDirectories(outputDir).ToList().ForEach(f => Directory.Delete(f, true));
        }

        /// <summary>
        /// Prerequisites next to JuliHelperLab.exe:
        /// -MGCB folder with the MGCB.exe and dependencies
        /// -JuliPipeline.dll (+JuliHelper.dll)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="importer">"FbxImporter"</param>
        /// <param name="processor"></param>
        /// <returns></returns>
        public static T LoadAsset<T>(string filePath, string importer, string processor, bool compiledEffect, string additionalArguments = "", params string[] processorParams)
        {
            ClearDynamicContentFolder();


            string paramString = "";
            for (int i = 0; i < processorParams.Length; i++)
            {
                paramString += " /processorParam:" + processorParams[i];
            }

            Process process = new Process();

            process.StartInfo.FileName = mgcbPath;
            process.StartInfo.Arguments =
                string.Format("/workingDir:\"" + Path.GetDirectoryName(filePath) + "\""
                + " /outputDir:\"" + outputDir + "\""
                + " /intermediateDir:\"" + intermediateDir + "\""
                + " /reference:\"" + G.exeDir + G.I + "JuliPipeline.dll\""
                + " /importer:" + importer
                + " /processor:" + processor
                + paramString
                + (compiledEffect ? (" /processorParam:CompiledEffectPath=\"" + effectCopyPath + "\" ") : "")
                + " " + additionalArguments
                + " /build:" + Path.GetFileName(filePath));


            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);

            Console.WriteLine("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);

            // Fire off the process.
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();


            //var content = new BuildContent();
            T t = ContentLoader.content.Load<T>("dynamic/" + Path.GetFileNameWithoutExtension(filePath));

            return t;
        }
    }
}
