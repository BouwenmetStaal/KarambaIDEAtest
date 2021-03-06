﻿// Copyright (c) 2019 Rayaan Ajouz, Bouwen met Staal. Please see the LICENSE file	
// for details. All rights reserved. Use of this source code is governed by a	
// Apache-2.0 license that can be found in the LICENSE file.	 
using KarambaIDEA.Core;
using KarambaIDEA.IDEA;

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using KarambaIDEA;
using System.IO;
//using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
using System.Globalization;
using System.Windows.Threading;

namespace Tester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            




            //TESTCalculate();
            //TESTCreateAndCalculateTemplate();
            //TESTCopyProject();
            try
            {
                RegistryKey staticaRoot = Registry.LocalMachine.OpenSubKey("SOFTWARE\\IDEAStatiCa");
                string[] SubKeyNames = staticaRoot.GetSubKeyNames();
                Dictionary<double?, string> versions = new Dictionary<double?, string>();
                foreach (string SubKeyName in SubKeyNames)
                {
                    versions.Add(double.Parse(SubKeyName, CultureInfo.InvariantCulture.NumberFormat), SubKeyName);
                }
                double[] staticaVersions = staticaRoot.GetSubKeyNames().Select(x => double.Parse(x, CultureInfo.InvariantCulture.NumberFormat)).OrderByDescending(x => x).ToArray();
                double? lastverion = staticaVersions.FirstOrDefault();
                string versionString = versions[lastverion];
                if (lastverion == null) { throw new ArgumentNullException("IDEA StatiCa installation cannot be found"); }
                string path = $@"{versionString.Replace(",", ".")}\IDEAStatiCa\Designer";
                string staticaFolderPath = staticaRoot.OpenSubKey(path).GetValue("InstallDir64").ToString();
            }
            catch
            {
                throw new ArgumentNullException("IDEA StatiCa installation cannot be found");
            }
            



            TESTCreateAndCalculate();


        }

        static void TESTCalculate()
        {
            // Initialize idea references, before calling code.
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(KarambaIDEA.IDEA.Utils.IdeaResolveEventHandler);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(KarambaIDEA.IDEA.Utils.IdeaResolveEventHandler);


            Joint joint = new Joint();
            joint.JointFilePath = "C:\\Data\\20191115214919\\C12-brandname\\APIproduced File - NotCorrect.ideaCon";
            HiddenCalculationV20.Calculate(joint);
            //Results
            string results = joint.ResultsSummary.summary;
        }
        static void TESTCreateAndCalculate()
        {
            Tester.GenerateTestJoint testrun = new GenerateTestJoint();

            //Define testjoint
            Joint joint = testrun.Testjoint2();


            //Define workshop operations
            joint.template = new Template();
            joint.template.workshopOperations = Template.WorkshopOperations.WeldAllMembers;

            //Set Project folder path
            string folderpath = @"C:\Data\";
            joint.project.CreateFolder(folderpath);

            //Set Joint folder path
            //string filepath = joint.project.projectFolderPath + ".ideaCon";
            //string fileName = joint.Name + ".ideaCon";
            //string jointFilePath = Path.Combine(joint.project.projectFolderPath, joint.Name, fileName);
            //joint.JointFilePath = jointFilePath;
            joint.JointFilePath = "xx";

            // Initialize idea references, before calling code.
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(KarambaIDEA.IDEA.Utils.IdeaResolveEventHandler);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(KarambaIDEA.IDEA.Utils.IdeaResolveEventHandler);

            //Create IDEA file
            IdeaConnection ideaConnection = new IdeaConnection(joint);

            //Calculate
            //HiddenCalculationV20.Calculate(joint);
            //KarambaIDEA.IDEA.HiddenCalculation main = new HiddenCalculation(joint);

            //Results
            //string results = joint.ResultsSummary.summary;
        }

        static void TESTCreateAndCalculateTemplate()
        {
            Tester.GenerateTestJoint testrun = new GenerateTestJoint();

            //Define testjoint
            Joint joint = testrun.Testjoint2();


            //Define Template location
            joint.ideaTemplateLocation = @"C:\SMARTconnection\BouwenmetStaal\KarambaIDEA\0_IDEA_Templates\TESTjointTester.contemp";

            //Set Project folder path
            string folderpath = @"C:\Data\";
            joint.project.CreateFolder(folderpath);

            //Set Joint folder path
            //string filepath = joint.project.projectFolderPath + ".ideaCon";
            //string fileName = joint.Name + ".ideaCon";
            //string jointFilePath = Path.Combine(joint.project.projectFolderPath, joint.Name, fileName);
            //joint.JointFilePath = jointFilePath;
            joint.JointFilePath = "xx";

            // Initialize idea references, before calling code.
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(KarambaIDEA.IDEA.Utils.IdeaResolveEventHandler);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(KarambaIDEA.IDEA.Utils.IdeaResolveEventHandler);

            //Create IDEA file
            IdeaConnection ideaConnection = new IdeaConnection(joint);

            //Calculate
            HiddenCalculationV20.Calculate(joint);
            

            //Results
            string results = joint.ResultsSummary.summary;
        }

        static void TESTCopyProject()
        {
            TestClass par = new TestClass();
            TestClass self = new TestClass() { parent = par, mypro = "sdsd" };

            par.children.Add(self);




        }

        public static void SetParent<T>(this T source, string propertyname, dynamic parent) where T : class
        {
            PropertyInfo pinfo = source.GetType().GetProperty(propertyname);
            if (pinfo != null)
            {
                pinfo.SetValue(source, parent);
            }
        }

       


    }
    public class TestClass
    {
        
        [NonSerialized]//toepassen project field
        public TestClass parent;
        public string mypro { get; set; }
        public List<TestClass> children { get; set; } = new List<TestClass>();
    }
}
