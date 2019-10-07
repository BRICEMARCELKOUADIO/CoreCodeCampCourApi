﻿using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>().ReverseMap();
            
                //.ForMember(a => a.VenueName, o => o.MapFrom(m => m.Location.VenueName));
        }
    }
}
