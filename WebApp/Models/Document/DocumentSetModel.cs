﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using LASI.Core;

namespace LASI.WebApp.Models
{
    public class DocumentSetModel : TextualModel<IEnumerable<Core.Document>>
    {
        public DocumentSetModel(IEnumerable<Core.Document> documents) : base(documents) {
            DocumentModels = documents.Select(document => new DocumentModel(document));
            foreach (var model in DocumentModels) { model.DocumentSetModel = this; }
        }
        public IEnumerable<DocumentModel> DocumentModels { get; private set; }

        public override Style Style { get { return new Style { CssClass = "documentlist" }; } }

        public override string Text { get { return ModelFor.Format(documentModel => documentModel.Text); } }

    }
}