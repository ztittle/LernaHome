using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Storage;
using ZWaveControllerClient;
using ZWaveControllerClient.Serial;

namespace LernaHome.Controllers
{
    [Route("api/nodes")]
    public class ZWaveNodeController : ControllerBase
    {
        private IUpdateableStorage _tripleStore;
        private ZWaveSerialController _zwaveController;

        public ZWaveNodeController(IUpdateableStorage tripleStore, ZWaveSerialController zwaveController)
        {
            _tripleStore = tripleStore;
            _zwaveController = zwaveController;
        }

        [HttpGet]
        public IGraph Get()
        {
            var g = (IGraph)_tripleStore.Query(@"
CONSTRUCT { ?s ?p ?o }
WHERE {
    GRAPH ?g {
        ?s ?p ?o
    }
}
");
            return g;
        }

        [HttpGet("id/{nodeId}")]
        public IGraph GetNode(int nodeId)
        {
            var g = (IGraph)_tripleStore.Query(@"
CONSTRUCT { ?s ?p ?o }
WHERE {
    GRAPH ?g {
        ?s ?p ?o
    }
}
");
            return g;
        }

        [HttpGet("id/{nodeId}/event/id/{eventId}")]
        public IGraph GetNodeEvent(int nodeId, int eventId)
        {
            var g = (IGraph)_tripleStore.Query(@"
CONSTRUCT { ?s ?p ?o }
WHERE {
    GRAPH ?g {
        ?s ?p ?o
    }
}
");
            return g;
        }

        [HttpGet("id/{nodeId}/event/id/{eventId}/parameter/id/{paramId}")]
        public IGraph GetNodeEventParameter(int nodeId, int eventId, string paramId)
        {
            var g = (IGraph)_tripleStore.Query(@"
CONSTRUCT { ?s ?p ?o }
WHERE {
    GRAPH ?g {
        ?s ?p ?o
    }
}
");
            return g;
        }

        [HttpPut("id/{nodeId}/event/id/{eventId}/parameter/id/{paramId}")]
        public async Task<IGraph> ReplaceNodeEventParameter(byte nodeId, int eventId, string paramId, [FromBody] Graph graph)
        {
            var valueNode = (ILiteralNode)graph.Triples.First().Object;

            var node = _zwaveController.Nodes.Single(n => n.Id == nodeId);

            var cmdClass = node.SupportedCommandClasses.First(c => c.Name == "COMMAND_CLASS_SWITCH_MULTILEVEL");
            var cmdSet = cmdClass.Commands.First(c => c.Name == "SWITCH_MULTILEVEL_SET");
            var cmdGet = cmdClass.Commands.First(c => c.Name == "SWITCH_MULTILEVEL_GET");

            var f = await _zwaveController.SendCommand(node, cmdClass, cmdSet, TransmitOptions.Acknowledge, byte.Parse(valueNode.Value));
            var f2 = await _zwaveController.SendCommand(node, cmdClass, cmdGet, TransmitOptions.Acknowledge);

            return graph;
        }
    }
}
