﻿// // //------------------------------------------------------------------------------------------------- 
// // // <copyright file="ModelController.cs" company="Microsoft Corporation">
// // // Copyright (c) Microsoft Corporation. All rights reserved.
// // // </copyright>
// // //-------------------------------------------------------------------------------------------------

namespace PyriteServer.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using System;
    using PyriteServer.Contracts;
    using PyriteServer.Results;

    public class ModelController : ApiController
    {
        [HttpGet]
        [Route("sets/{setid}/{version}/models/{detailLevel}/{xpos},{ypos},{zpos}")]
        [CacheControl(336 * 60)]
        public async Task<IHttpActionResult> Get(string setid, string version, string detailLevel, string xpos, string ypos, string zpos, string fmt = null)
        {
            try
            {
                StorageStream modelStream = await Dependency.Storage.GetModelStream(setid, version, detailLevel, xpos, ypos, zpos, fmt);
                return new StreamResult(modelStream, this.Request);
            }
            catch (NotFoundException)
            {
                return this.NotFound();
            }
        }
    }
}