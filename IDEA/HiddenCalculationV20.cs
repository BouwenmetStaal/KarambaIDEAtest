﻿using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using IdeaRS.OpenModel.Geometry3D;

using KarambaIDEA.Core;
using System.Linq;
using System.Windows.Forms;
using System;

namespace KarambaIDEA.IDEA
{
    
    /// <summary>
    /// Main view model of the example
    /// </summary>
    /// //public class HiddenCalculation : INotifyPropertyChanged, IConHiddenCalcModel
    public class HiddenCalculationV20
    {
        public static void Calculate(Joint joint)
        {
            string path = IdeaConnection.ideaStatiCaDir;//path to idea
            string pathToFile = joint.JointFilePath;//ideafile path
            string newBoltAssemblyName = "M16 8.8";
            var calcFactory = new ConnHiddenClientFactory(path);
            ConnectionResultsData conRes = null;
            var client = calcFactory.Create();
            try
            {
                client.OpenProject(pathToFile);


                try
                {

                    // get detail about idea connection project
                    var projInfo = client.GetProjectInfo();

                    var connection = projInfo.Connections.FirstOrDefault();//Select first connection
                    if (joint.ideaTemplateLocation != null)
                    {
                        client.AddBoltAssembly(newBoltAssemblyName);//??Here Martin

                        client.ApplyTemplate(connection.Identifier, joint.ideaTemplateLocation, null);
                        client.SaveAsProject(pathToFile);
                    }


                    conRes = client.Calculate(connection.Identifier);
                    client.SaveAsProject(pathToFile);
                    //projInfo.Connections.Count()
                    if (projInfo != null && projInfo.Connections != null)
                    {

                        /*
                        // iterate all connections in the project
                        foreach (var con in projInfo.Connections)
                        {
                            //Console.WriteLine(string.Format("Starting calculation of connection {0}", con.Identifier));

                            // calculate a get results for each connection in the project
                            var conRes = client.Calculate(con.Identifier);
                            //Console.WriteLine("Calculation is done");

                            // get the geometry of the connection
                            var connectionModel = client.GetConnectionModel(con.Identifier);
                        }
                        */
                    }
                }
                finally
                {
                    // Delete temps in case of a crash
                    client.CloseProject();
                }
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
            if (conRes != null)
            {
                IdeaConnection.SaveResultsSummary(joint, conRes);
            }
        }

  
    }
    public static class CalculationExtentions
    {
        public static double? GetResult(this List<CheckResSummary> source, string key  )
        {
            var boltResult = source.FirstOrDefault(x => x.Name == key);
            if (boltResult != null)
            { 
                return boltResult.CheckValue; 
            }
            return null;
        }

        public static Vector3D Unitize(this Vector3D vec)
        {
            vec.X = vec.X / vec.Length();
            vec.Y = vec.Y / vec.Length();
            vec.Z = vec.Z / vec.Length();
            return vec;
        }

        public static double Length(this Vector3D vec)
        {

            return Math.Sqrt(Math.Pow(vec.X, 2) + Math.Pow(vec.Y, 2) + Math.Pow(vec.Z, 2));
        }

        static public Vector3D VecScalMultiply(this Vector3D vec, double scalar)
        {
            Vector3D vector = new Vector3D();
            vector.X = vec.X * scalar;
            vector.Y = vec.Y * scalar;
            vector.Z = vec.Z * scalar;
            return vector;
        }

        public static Point3D MovePointVecAndLength(this Point3D point, Vector3D vec, double length)
        {
            vec.Unitize();
            Vector3D move = vec.VecScalMultiply(length);
            Point3D newpoint = new Point3D();
            newpoint.X = point.X + move.X;
            newpoint.Y = point.Y + move.Y;
            newpoint.Z = point.Z + move.Z;

            return newpoint;
        }
    }

    
}

