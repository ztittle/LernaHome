using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Storage;
using System.Threading.Tasks;
using ZWaveControllerClient.Serial;
using ZWaveControllerClient;
using ZWaveControllerClient.CommandClasses;

namespace LernaHome.ZWave
{
    public class ZWaveTripleCollector
    {
        private ZWaveSerialController _zwaveController;
        private IUpdateableStorage _tripleStore;
        public ZWaveTripleCollector(ZWaveSerialController zwaveController, IUpdateableStorage tripleStore)
        {
            _zwaveController = zwaveController;
            _zwaveController.FrameReceived += _zwaveController_FrameReceived;
            _tripleStore = tripleStore;
        }

        private void _zwaveController_FrameReceived(object sender, FrameEventArgs e)
        {
            switch (e.Frame.Function)
            {
                case ZWaveFunction.ApplicationUpdate:
                    SaveNodesToStore();
                    break;
                case ZWaveFunction.ApplicationCommandHandler:
                    break;
            }
        }

        public void SaveNodesToStore()
        {
            var graph = new Graph()
                .SetDefaultNamespaces();

            graph.BaseUri = new Uri(UriHelper.BuildAbsolute("http", new Microsoft.AspNetCore.Http.HostString("localhost:5000")));
            var subject = new Uri(graph.BaseUri, "api/nodes");

            var zwNodes = _zwaveController.Nodes;

            graph.AddSubject(subject)
                .WithPredicate("rdf:type")
                .AndNamedObject("hydra:Collection")
                .Assert();

            graph.AddSubject(subject)
                .WithPredicate("hydra:totalItems")
                .AndLiteralObject(zwNodes.Count.ToString(), "xsd:nonNegativeNumber")
                .Assert();

            foreach (var zwNode in zwNodes)
            {
                var zwNodeSubject = new Uri($"{subject}/id/{zwNode.Id}");
                graph.AddSubject(subject)
                    .WithPredicate("hydra:member")
                    .AndNamedObject(zwNodeSubject)
                    .Assert();

                graph.AddSubject(zwNodeSubject)
                    .WithPredicate("rdf:type")
                    .AndNamedObject($"zwave:Node")
                    .Assert();

                graph.AddSubject(zwNodeSubject)
                    .WithPredicate("zwave:hasGenericType")
                    .AndNamedObject($"zwave:{zwNode.ProtocolInfo.GenericType.Name}")
                    .Assert();

                graph.AddSubject($"zwave:{zwNode.ProtocolInfo.GenericType.Name}")
                    .WithPredicate("zwave:hasValue")
                    .AndLiteralObject($"{zwNode.ProtocolInfo.GenericType.Key:x02}", "xsd:hexBinary")
                    .Assert();

                graph.AddSubject($"zwave:{zwNode.ProtocolInfo.GenericType.Name}")
                    .WithPredicate("rdfs:label")
                    .AndLiteralObject($"{zwNode.ProtocolInfo.GenericType.Help}")
                    .Assert();

                graph.AddSubject(zwNodeSubject)
                    .WithPredicate("zwave:hasSpecificType")
                    .AndNamedObject($"zwave:{zwNode.ProtocolInfo.SpecificType.Name}")
                    .Assert();

                graph.AddSubject($"zwave:{zwNode.ProtocolInfo.SpecificType.Name}")
                    .WithPredicate("rdfs:label")
                    .AndLiteralObject($"{zwNode.ProtocolInfo.SpecificType.Help}")
                    .Assert();

                graph.AddSubject($"zwave:{zwNode.ProtocolInfo.SpecificType.Name}")
                    .WithPredicate("zwave:hasValue")
                    .AndLiteralObject($"{zwNode.ProtocolInfo.SpecificType.Key:x02}", "xsd:hexBinary")
                    .Assert();

                foreach (var commandClass in zwNode.SupportedCommandClasses)
                {
                    var commandClassNode = commandClass.Version == null
                        ? $"zwave:CommandClass/{commandClass.Name}"
                        : $"zwave:CommandClass/{commandClass.Name}/version/{commandClass.Version}";

                    graph.AddSubject(zwNodeSubject)
                        .WithPredicate("zwave:hasSupportedCommandClass")
                        .AndNamedObject(commandClassNode)
                        .Assert();

                    graph.AddSubject(commandClassNode)
                        .WithPredicate("rdf:type")
                        .AndNamedObject($"zwave:CommandClass")
                        .Assert();

                    if (commandClass.Version != null)
                    {
                        graph.AddSubject(commandClassNode)
                            .WithPredicate("zwave:hasVersion")
                            .AndLiteralObject($"{commandClass.Version}", "xsd:Integer")
                            .Assert();
                    }

                    graph.AddSubject(commandClassNode)
                        .WithPredicate("rdfs:label")
                        .AndLiteralObject($"{commandClass.Help}")
                        .Assert();

                    graph.AddSubject(commandClassNode)
                        .WithPredicate("zwave:hasValue")
                        .AndLiteralObject($"{commandClass.Key:x02}", "xsd:hexBinary")
                        .Assert();

                    foreach(var cmd in commandClass.Commands)
                    {
                        graph.AddSubject(commandClassNode)
                            .WithPredicate("zwave:hasCommand")
                            .AndNamedObject($"zwave:Command/{cmd.Name}")
                            .Assert();

                        graph.AddSubject($"zwave:Command/{cmd.Name}")
                            .WithPredicate("rdfs:label")
                            .AndLiteralObject(cmd.Help)
                            .Assert();

                        foreach (var cmdParam in cmd.Parameters)
                        {
                            graph.AddSubject($"zwave:Command/{cmd.Name}")
                                .WithPredicate("zwave:hasParameter")
                                .AndNamedObject($"zwave:CommandParameter/{cmdParam.Name}")
                                .Assert();

                            graph.AddSubject($"zwave:CommandParameter/{cmdParam.Name}")
                                .WithPredicate("zwave:hasValue")
                                .AndLiteralObject($"{cmdParam.Key:x02}", "xsd:hexBinary")
                                .Assert();
                        }
                    }
                }
            }

            _tripleStore.SaveGraph(graph);
        }
    }
}
