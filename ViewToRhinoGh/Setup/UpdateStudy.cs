// using System;
// using System.Collections.Generic;
// using System.Drawing;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using ConnectorGrasshopper.Extras;
// using ConnectorGrasshopper.Objects;
// using Grasshopper.Kernel;
// using Grasshopper.Kernel.Types;
// using Rhino;
// using Speckle.Core.Api;
// using Speckle.Core.Credentials;
// using Speckle.Core.Kits;
// using Speckle.Core.Transports;
// using ViewTo;
// using ViewTo.Speckle.ViewObject;
// using ViewToRhinoGh.Goo;
// using ViewToRhinoGh.Properties;
// using Pipe = ViewToRhinoGh.ConnectorPipe;
//
// namespace ViewToRhinoGh.Setup
// {
//   
//   public class UpdateStudy : GH_TaskCapableComponent<List<StreamWrapper>>
//   {
//
//     public override Guid ComponentGuid => new Guid("0BD185C5-480B-43CA-959F-CDD0D11BAB59");
//
//     protected override Bitmap Icon => new Bitmap(Resources.UploadViewStudy);
//
//     public override GH_Exposure Exposure => GH_Exposure.primary;
//
//     public UpdateStudy() : base("UpdateStudy", "UP",
//                                 "Update a View Study with new content and points. This goes right to speckle",
//                                 ConnectorInfo.CATEGORY,
//                                 ConnectorInfo.Nodes.STUDY)
//     { }
//
//     private int _iStudy, _iStream, _iMsg;
//     protected override void RegisterInputParams(GH_InputParamManager pManager)
//     {
//       var i = 0;
//       pManager.AddGenericParameter("Data", "D", "The data to send.", GH_ParamAccess.item);
//       _iStudy = i++;
//       pManager.AddGenericParameter("Stream", "S", "Stream(s) and/or transports to send to.", GH_ParamAccess.item);
//       _iStream = i++;
//       pManager.AddTextParameter("Message", "M", "Commit message. If left blank, one will be generated for you.", GH_ParamAccess.item, "");
//       _iMsg = i;
//       Params.Input[_iMsg].Optional = true;
//     }
//
//     protected override void RegisterOutputParams(GH_OutputParamManager pManager)
//     {
//       pManager.AddGenericParameter("Commit ID", "C", "Stream or streams pointing to the created commit", GH_ParamAccess.list);
//     }
//
//     private ViewStudy study;
//     public List<StreamWrapper> OutputWrappers { get; private set; } = new List<StreamWrapper>();
//     protected override void SolveInstance(IGH_DataAccess DA)
//     {
//       DA.DisableGapLogic();
//
//       if (RunCount == 1)
//       {
//         OutputWrappers = new List<StreamWrapper>();
//         Pipe.SetDoc(RhinoDoc.ActiveDoc);
//
//         var wrapper = new object();
//         DA.GetData(_iStudy, ref wrapper);
//
//         if (wrapper is GH_ViewObj @object && @object.Value is ViewStudy viewObj)
//           study = viewObj;
//       }
//
//       if (InPreSolve)
//       {
//         if (study == null || !(Pipe.ToSpeckle(study) is ViewStudyBase @base)) return;
//
//         var stream = new GH_ObjectWrapper();
//         DA.GetData(_iStream, ref stream);
//         
//         var msg = new GH_String();
//         DA.GetData(_iMsg, ref msg);
//
//         var task = Task.Run(async () =>
//         {
//           var unwrappedTransport = stream.GetType().GetProperty("Value").GetValue(stream);
//           if (!(unwrappedTransport is GH_SpeckleStream ghStream) || !(ghStream.Value is StreamWrapper sw)) return null;
//
//           var prevCommits = new List<StreamWrapper>();
//           try
//           {
//             var acc = sw.GetAccount().Result;
//             var transport = new ServerTransport(acc, sw.StreamId);
//             var baseID = await Operations.Send(@base, new List<ITransport> {transport});
//             var client = new Client(transport.Account);
//
//             var commitCreateInput = new CommitCreateInput
//             {
//               branchName = "main",
//               message = msg != null && !string.IsNullOrEmpty(msg.Value) ? msg.Value : "up:" + @base.viewName,
//               objectId = baseID,
//               streamId = transport.StreamId,
//               sourceApplication = Applications.Grasshopper
//             };
//
//
//             var prevCommit = prevCommits.FirstOrDefault(c => c.ServerUrl == client.ServerUrl && c.StreamId == transport.StreamId);
//             if (prevCommit != null)
//               commitCreateInput.parents = new List<string>() {prevCommit.CommitId};
//
//             var commitId = await client.CommitCreate(commitCreateInput);
//             var streamWrapper =
//               new StreamWrapper($"{client.Account.serverInfo.url}/streams/{transport.StreamId}/commits/{commitId}?u={client.Account.userInfo.id}");
//
//             prevCommits.Add(streamWrapper);
//           }
//           catch (Exception e)
//           {
//             Console.WriteLine(e);
//             return null;
//           }
//
//
//           return prevCommits;
//         }, CancellationToken.None);
//         TaskList.Add(task);
//         return;
//       }
//       if (!GetSolveResults(DA, out List<StreamWrapper> outputWrappers))
//         AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not running multithread");
//       else
//       {
//         OutputWrappers.AddRange(outputWrappers);
//         DA.SetDataList(0, outputWrappers);
//       }
//     }
//   }
// }
