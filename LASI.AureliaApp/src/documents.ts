import {autoinject, bindable} from 'aurelia-framework';
import DocumentService from './services/document-model-service';
import { DocumentModel } from 'models';

@autoinject export class Documents {
  constructor(readonly documentModelService: DocumentService) { }

  async activate() {
    this.document = await this.documentModelService.processDocument(4);
  }

  @bindable document: DocumentModel;
}