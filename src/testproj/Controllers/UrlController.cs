﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using testproj.Contexts;
using testproj.Models;
using testsite.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace testproj.Controllers
{
    [Route("api/[controller]")]
    public class UrlController : Controller
    {
        private ITableContext _tableContext;

        public UrlController(ITableContext context)
        {
            _tableContext = context;
        }

        // GET: api/Url
        [HttpGet]
        public async Task<IEnumerable<KeyValuePair<string, string>>> Get()
        {
            var ret = await _tableContext.RetrieveUrl();
            return ret.ToList();
        }

        // GET api/Url/5
        [HttpGet("{identifier}")]
        public async Task<KeyValuePair<string, string>> Get(string identifier)
        {
            var ret = await _tableContext.ResolverUrl(identifier);
            return new KeyValuePair<string, string>(identifier, ret);
        }

        // POST api/Url
        [HttpPost]
        public async Task<KeyValuePair<string, string>> Post([FromBody]UrlCreateViewModel url)
        {
            var ret = await _tableContext.CreateUrl(url.Value);
            return new KeyValuePair<string, string>(ret, url.Value);

        }

        // PUT api/Url/5
        [HttpPut("{identifier}")]
        public KeyValuePair<string, string> Put([FromBody]UrlEditViewModel url)
        {
            return new KeyValuePair<string, string>(url.Key, url.Value);
        }
    }
}
