﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using LASI.Core;
using LASI.Utilities;

namespace LASI.WebApp.Old.Models
{
    public class DocumentSetModel : TextualModel<IEnumerable<Document>>
    {
        public DocumentSetModel(IEnumerable<Document> documents) : base(documents)
        {
            DocumentModels = documents.Select(document => new DocumentModel(document));
            foreach (var model in DocumentModels) { model.DocumentSetModel = this; }
        }
        public IEnumerable<DocumentModel> DocumentModels { get; }

        public override Style Style => new Style { CssClass = "documentlist" };
        public override string Text => ModelFor.Format(documentModel => documentModel.Text);

    }
}