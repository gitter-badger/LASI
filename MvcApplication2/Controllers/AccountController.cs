﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LASI.WebService.Models;
using LASI.WebService.Models.User;
using Newtonsoft.Json;

namespace LASI.WebService.Controllers
{
    public class AccountController : Controller
    {


        public ActionResult Login() {
            return View();
        }


        public ActionResult CreateAccount() {
            return View(new UserModel());
        }
        [HttpPost]
        public ActionResult CreateNew(UserModel model) {
            var settings = new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            var userDataFile = Server.MapPath("~/App_Data/Users/" + model.Email + ".json");
            using (var writer = new JsonTextWriter(new System.IO.StreamWriter(userDataFile, append: true)) { Formatting = Formatting.Indented }) {
                JsonSerializer.Create(settings).Serialize(writer, model);
            }


            return RedirectToAction("Index", "Home");
        }

    }
}