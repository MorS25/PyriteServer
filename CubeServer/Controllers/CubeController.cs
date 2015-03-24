﻿// // //------------------------------------------------------------------------------------------------- 
// // // <copyright file="CubeController.cs" company="Microsoft Corporation">
// // // Copyright (c) Microsoft Corporation. All rights reserved.
// // // </copyright>
// // //-------------------------------------------------------------------------------------------------

namespace CubeServer.Controllers
{
    using System.Collections.Generic;
    using System.Web.Http;

    public class CubeController : ApiController
    {
        [HttpGet]
        [Route("sets/{setid}/{version}/cubes/{detail}/{x:float},{y:float},{z:float},{h:float},{w:float},{d:float}")]
        public IEnumerable<object> Get(string setid, string version, string detail, float x, float y, float z, float h, float w, float d)
        {
            return new string[] { "cube1", "cube2" };
        }
    }
}