﻿using System;
using System.Net.Http;

using BouvetCodeCamp.Domene.OutputModels;
using Newtonsoft.Json;
using Quartz;

namespace BouvetCodeCamp.SpillSimulator.Jobs
{
    public class BaseGjeldendePostJob : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            string ApiEndPointAddress = SpillKonfig.ApiBaseAddress + "/api/game/base/hentgjeldendepost/" + SpillKonfig.LagId;

            PostOutputModell gjeldendePost;

            using (var httpClient = new HttpClient())
            {
                var httpResponseMessage = await httpClient.GetAsync(ApiEndPointAddress);
                var content = await httpResponseMessage.Content.ReadAsStringAsync();

                SpillKonfig.GjeldendePost = JsonConvert.DeserializeObject<PostOutputModell>(content);

                if (SpillKonfig.GjeldendePost == null)
                    Console.WriteLine("BASE Ingen flere poster å hente.");

                Console.WriteLine("BASE Hentet ny gjeldende post med nummer {0}", SpillKonfig.GjeldendePost.Nummer);
            }
        }
    }
}
