// Generated by typings
// Source: https://raw.githubusercontent.com/aurelia/templating-router/1.0.0/dist/aurelia-templating-router.d.ts
declare module 'aurelia-templating-router' {
import * as LogManager from 'aurelia-logging';
import {
  customAttribute,
  bindable,
  ViewSlot,
  ViewLocator,
  customElement,
  noView,
  BehaviorInstruction,
  CompositionTransaction,
  CompositionEngine,
  ShadowDOM
} from 'aurelia-templating';
import {
  inject,
  Container
} from 'aurelia-dependency-injection';
import {
  Router,
  RouteLoader
} from 'aurelia-router';
import {
  DOM
} from 'aurelia-pal';
import {
  Origin
} from 'aurelia-metadata';
import {
  relativeToFile
} from 'aurelia-path';
export class RouteHref {
  constructor(router?: any, element?: any);
  bind(): any;
  unbind(): any;
  attributeChanged(value?: any, previous?: any): any;
  processChange(): any;
}
export class RouterView {
  swapOrder: any;
  layoutView: any;
  layoutViewModel: any;
  layoutModel: any;
  constructor(element?: any, container?: any, viewSlot?: any, router?: any, viewLocator?: any, compositionTransaction?: any, compositionEngine?: any);
  created(owningView?: any): any;
  bind(bindingContext?: any, overrideContext?: any): any;
  process(viewPortInstruction?: any, waitToSwap?: any): any;
  swap(viewPortInstruction?: any): any;
}
export class TemplatingRouteLoader extends RouteLoader {
  constructor(compositionEngine?: any);
  loadRoute(router?: any, config?: any): any;
}
}
