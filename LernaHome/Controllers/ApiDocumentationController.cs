using Microsoft.AspNetCore.Mvc;
using System;
using VDS.RDF;

namespace LernaHome.Controllers
{
    [Route("api/doc")]
    public class ApiDocumentationController : ControllerBase
    {
        [HttpGet]
        [Route("", Name = Routes.ApiDocs.Get)]
        public IGraph Get()
        {
            var apiDocGraph = new Graph
            {
                BaseUri = new Uri(Url.Link(Routes.ApiDocs.Get, null))
            };

            apiDocGraph.SetDefaultNamespaces();

            apiDocGraph.AddSubject(apiDocGraph.BaseUri)
                .WithPredicate("rdf:type")
                .AndNamedObject("hydra:ApiDocumentation")
                .Assert();

            apiDocGraph.AddSubject(apiDocGraph.BaseUri)
                .WithPredicate("hydra:title")
                .AndLiteralObject("Lerna Home Automation API")
                .Assert();

            apiDocGraph.AddSubject(apiDocGraph.BaseUri)
                .WithPredicate("hydra:description")
                .AndLiteralObject("Something witty")
                .Assert();

            apiDocGraph.AddSubject(apiDocGraph.BaseUri)
                .WithPredicate("hydra:entrypoint")
                .AndNamedObject(new Uri(Url.Link(Routes.Nodes.Get, null)))
                .Assert();

            return apiDocGraph;
        }
    }
}
