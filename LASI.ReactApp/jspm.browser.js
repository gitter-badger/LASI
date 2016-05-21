SystemJS.config({
  baseURL: "/",
  trace: true,
  paths: {
    "github:*": "jspm_packages/github/*",
    "npm:*": "jspm_packages/npm/*",
    "lasi-react/": "src/"
  },
  bundles: {
    "build.js": [
      "lasi-react/main.tsx",
      "lasi-react/app.tsx",
      "lasi-react/document-viewer/document-viewer.tsx",
      "lasi-react/document-viewer/paragraph.tsx",
      "lasi-react/document-viewer/sentence.tsx",
      "lasi-react/document-viewer/phrase.tsx",
      "npm:react@15.0.2/react.js",
      "npm:react@15.0.2.json",
      "github:jspm/nodelibs-process@0.2.0-alpha/process.js",
      "github:jspm/nodelibs-process@0.2.0-alpha.json",
      "npm:react@15.0.2/lib/React.js",
      "npm:fbjs@0.8.2/lib/warning.js",
      "npm:fbjs@0.8.2.json",
      "npm:fbjs@0.8.2/lib/emptyFunction.js",
      "npm:react@15.0.2/lib/onlyChild.js",
      "npm:fbjs@0.8.2/lib/invariant.js",
      "npm:react@15.0.2/lib/ReactElement.js",
      "npm:react@15.0.2/lib/canDefineProperty.js",
      "npm:react@15.0.2/lib/ReactCurrentOwner.js",
      "npm:object-assign@4.1.0/index.js",
      "npm:object-assign@4.1.0.json",
      "npm:react@15.0.2/lib/ReactVersion.js",
      "npm:react@15.0.2/lib/ReactPropTypes.js",
      "npm:react@15.0.2/lib/getIteratorFn.js",
      "npm:react@15.0.2/lib/ReactPropTypeLocationNames.js",
      "npm:react@15.0.2/lib/ReactElementValidator.js",
      "npm:react@15.0.2/lib/ReactPropTypeLocations.js",
      "npm:fbjs@0.8.2/lib/keyMirror.js",
      "npm:react@15.0.2/lib/ReactDOMFactories.js",
      "npm:fbjs@0.8.2/lib/mapObject.js",
      "npm:react@15.0.2/lib/ReactClass.js",
      "npm:fbjs@0.8.2/lib/keyOf.js",
      "npm:fbjs@0.8.2/lib/emptyObject.js",
      "npm:react@15.0.2/lib/ReactNoopUpdateQueue.js",
      "npm:react@15.0.2/lib/ReactComponent.js",
      "npm:react@15.0.2/lib/ReactInstrumentation.js",
      "npm:react@15.0.2/lib/ReactDebugTool.js",
      "npm:react@15.0.2/lib/ReactInvalidSetStateWarningDevTool.js",
      "npm:react@15.0.2/lib/ReactChildren.js",
      "npm:react@15.0.2/lib/traverseAllChildren.js",
      "npm:react@15.0.2/lib/KeyEscapeUtils.js",
      "npm:react@15.0.2/lib/PooledClass.js",
      "github:frankwallis/plugin-typescript@4.0.16.json",
      "github:twbs/bootstrap@3.3.6/dist/js/bootstrap.js",
      "github:twbs/bootstrap@3.3.6.json",
      "npm:jquery@2.2.3/dist/jquery.js",
      "npm:jquery@2.2.3.json",
      "github:twbs/bootstrap@3.3.6/dist/css/bootstrap.css!github:systemjs/plugin-css@0.1.21/css.js",
      "github:systemjs/plugin-css@0.1.21.json",
      "doc.json!github:systemjs/plugin-json@0.1.2/json.js",
      "github:systemjs/plugin-json@0.1.2.json",
      "npm:react-dom@15.0.2/index.js",
      "npm:react-dom@15.0.2.json",
      "npm:react@15.0.2/lib/ReactDOM.js",
      "npm:fbjs@0.8.2/lib/ExecutionEnvironment.js",
      "npm:react@15.0.2/lib/renderSubtreeIntoContainer.js",
      "npm:react@15.0.2/lib/ReactMount.js",
      "npm:react@15.0.2/lib/shouldUpdateReactComponent.js",
      "npm:react@15.0.2/lib/setInnerHTML.js",
      "npm:react@15.0.2/lib/createMicrosoftUnsafeLocalFunction.js",
      "npm:react@15.0.2/lib/instantiateReactComponent.js",
      "npm:react@15.0.2/lib/ReactNativeComponent.js",
      "npm:react@15.0.2/lib/ReactEmptyComponent.js",
      "npm:react@15.0.2/lib/ReactCompositeComponent.js",
      "npm:react@15.0.2/lib/ReactUpdateQueue.js",
      "npm:react@15.0.2/lib/ReactUpdates.js",
      "npm:react@15.0.2/lib/Transaction.js",
      "npm:react@15.0.2/lib/ReactReconciler.js",
      "npm:react@15.0.2/lib/ReactRef.js",
      "npm:react@15.0.2/lib/ReactOwner.js",
      "npm:react@15.0.2/lib/ReactPerf.js",
      "npm:react@15.0.2/lib/ReactFeatureFlags.js",
      "npm:react@15.0.2/lib/CallbackQueue.js",
      "npm:react@15.0.2/lib/ReactInstanceMap.js",
      "npm:react@15.0.2/lib/ReactNodeTypes.js",
      "npm:react@15.0.2/lib/ReactErrorUtils.js",
      "npm:react@15.0.2/lib/ReactComponentEnvironment.js",
      "npm:react@15.0.2/lib/ReactMarkupChecksum.js",
      "npm:react@15.0.2/lib/adler32.js",
      "npm:react@15.0.2/lib/ReactDOMFeatureFlags.js",
      "npm:react@15.0.2/lib/ReactDOMContainerInfo.js",
      "npm:react@15.0.2/lib/validateDOMNesting.js",
      "npm:react@15.0.2/lib/ReactDOMComponentTree.js",
      "npm:react@15.0.2/lib/ReactDOMComponentFlags.js",
      "npm:react@15.0.2/lib/DOMProperty.js",
      "npm:react@15.0.2/lib/ReactBrowserEventEmitter.js",
      "npm:react@15.0.2/lib/isEventSupported.js",
      "npm:react@15.0.2/lib/getVendorPrefixedEventName.js",
      "npm:react@15.0.2/lib/ViewportMetrics.js",
      "npm:react@15.0.2/lib/ReactEventEmitterMixin.js",
      "npm:react@15.0.2/lib/EventPluginHub.js",
      "npm:react@15.0.2/lib/forEachAccumulated.js",
      "npm:react@15.0.2/lib/accumulateInto.js",
      "npm:react@15.0.2/lib/EventPluginUtils.js",
      "npm:react@15.0.2/lib/EventConstants.js",
      "npm:react@15.0.2/lib/EventPluginRegistry.js",
      "npm:react@15.0.2/lib/DOMLazyTree.js",
      "npm:react@15.0.2/lib/setTextContent.js",
      "npm:react@15.0.2/lib/escapeTextContentForBrowser.js",
      "npm:react@15.0.2/lib/getNativeComponentFromComposite.js",
      "npm:react@15.0.2/lib/findDOMNode.js",
      "npm:react@15.0.2/lib/ReactDefaultInjection.js",
      "npm:react@15.0.2/lib/ReactDefaultPerf.js",
      "npm:fbjs@0.8.2/lib/performanceNow.js",
      "npm:fbjs@0.8.2/lib/performance.js",
      "npm:react@15.0.2/lib/ReactDefaultPerfAnalysis.js",
      "npm:react@15.0.2/lib/SimpleEventPlugin.js",
      "npm:react@15.0.2/lib/getEventCharCode.js",
      "npm:react@15.0.2/lib/SyntheticWheelEvent.js",
      "npm:react@15.0.2/lib/SyntheticMouseEvent.js",
      "npm:react@15.0.2/lib/getEventModifierState.js",
      "npm:react@15.0.2/lib/SyntheticUIEvent.js",
      "npm:react@15.0.2/lib/getEventTarget.js",
      "npm:react@15.0.2/lib/SyntheticEvent.js",
      "npm:react@15.0.2/lib/SyntheticTransitionEvent.js",
      "npm:react@15.0.2/lib/SyntheticTouchEvent.js",
      "npm:react@15.0.2/lib/SyntheticDragEvent.js",
      "npm:react@15.0.2/lib/SyntheticKeyboardEvent.js",
      "npm:react@15.0.2/lib/getEventKey.js",
      "npm:react@15.0.2/lib/SyntheticFocusEvent.js",
      "npm:react@15.0.2/lib/SyntheticClipboardEvent.js",
      "npm:react@15.0.2/lib/SyntheticAnimationEvent.js",
      "npm:react@15.0.2/lib/EventPropagators.js",
      "npm:fbjs@0.8.2/lib/EventListener.js",
      "npm:react@15.0.2/lib/SelectEventPlugin.js",
      "npm:fbjs@0.8.2/lib/shallowEqual.js",
      "npm:react@15.0.2/lib/isTextInputElement.js",
      "npm:fbjs@0.8.2/lib/getActiveElement.js",
      "npm:react@15.0.2/lib/ReactInputSelection.js",
      "npm:fbjs@0.8.2/lib/focusNode.js",
      "npm:fbjs@0.8.2/lib/containsNode.js",
      "npm:fbjs@0.8.2/lib/isTextNode.js",
      "npm:fbjs@0.8.2/lib/isNode.js",
      "npm:react@15.0.2/lib/ReactDOMSelection.js",
      "npm:react@15.0.2/lib/getTextContentAccessor.js",
      "npm:react@15.0.2/lib/getNodeForCharacterOffset.js",
      "npm:react@15.0.2/lib/SVGDOMPropertyConfig.js",
      "npm:react@15.0.2/lib/ReactReconcileTransaction.js",
      "npm:react@15.0.2/lib/ReactInjection.js",
      "npm:react@15.0.2/lib/ReactEventListener.js",
      "npm:fbjs@0.8.2/lib/getUnboundedScrollPosition.js",
      "npm:react@15.0.2/lib/ReactDefaultBatchingStrategy.js",
      "npm:react@15.0.2/lib/ReactDOMTextComponent.js",
      "npm:react@15.0.2/lib/DOMChildrenOperations.js",
      "npm:react@15.0.2/lib/ReactMultiChildUpdateTypes.js",
      "npm:react@15.0.2/lib/Danger.js",
      "npm:fbjs@0.8.2/lib/getMarkupWrap.js",
      "npm:fbjs@0.8.2/lib/createNodesFromMarkup.js",
      "npm:fbjs@0.8.2/lib/createArrayFromMixed.js",
      "npm:react@15.0.2/lib/ReactDOMTreeTraversal.js",
      "npm:react@15.0.2/lib/ReactDOMEmptyComponent.js",
      "npm:react@15.0.2/lib/ReactDOMComponent.js",
      "npm:react@15.0.2/lib/ReactMultiChild.js",
      "npm:react@15.0.2/lib/flattenChildren.js",
      "npm:react@15.0.2/lib/ReactChildReconciler.js",
      "npm:react@15.0.2/lib/ReactDOMTextarea.js",
      "npm:react@15.0.2/lib/LinkedValueUtils.js",
      "npm:react@15.0.2/lib/DOMPropertyOperations.js",
      "npm:react@15.0.2/lib/quoteAttributeValueForBrowser.js",
      "npm:react@15.0.2/lib/ReactDOMInstrumentation.js",
      "npm:react@15.0.2/lib/ReactDOMDebugTool.js",
      "npm:react@15.0.2/lib/ReactDOMUnknownPropertyDevtool.js",
      "npm:react@15.0.2/lib/DisabledInputUtils.js",
      "npm:react@15.0.2/lib/ReactDOMSelect.js",
      "npm:react@15.0.2/lib/ReactDOMOption.js",
      "npm:react@15.0.2/lib/ReactDOMInput.js",
      "npm:react@15.0.2/lib/ReactDOMButton.js",
      "npm:react@15.0.2/lib/ReactComponentBrowserEnvironment.js",
      "npm:react@15.0.2/lib/ReactDOMIDOperations.js",
      "npm:react@15.0.2/lib/DOMNamespaces.js",
      "npm:react@15.0.2/lib/CSSPropertyOperations.js",
      "npm:fbjs@0.8.2/lib/memoizeStringOnly.js",
      "npm:fbjs@0.8.2/lib/hyphenateStyleName.js",
      "npm:fbjs@0.8.2/lib/hyphenate.js",
      "npm:react@15.0.2/lib/dangerousStyleValue.js",
      "npm:react@15.0.2/lib/CSSProperty.js",
      "npm:fbjs@0.8.2/lib/camelizeStyleName.js",
      "npm:fbjs@0.8.2/lib/camelize.js",
      "npm:react@15.0.2/lib/AutoFocusUtils.js",
      "npm:react@15.0.2/lib/HTMLDOMPropertyConfig.js",
      "npm:react@15.0.2/lib/EnterLeaveEventPlugin.js",
      "npm:react@15.0.2/lib/DefaultEventPluginOrder.js",
      "npm:react@15.0.2/lib/ChangeEventPlugin.js",
      "npm:react@15.0.2/lib/BeforeInputEventPlugin.js",
      "npm:react@15.0.2/lib/SyntheticInputEvent.js",
      "npm:react@15.0.2/lib/SyntheticCompositionEvent.js",
      "npm:react@15.0.2/lib/FallbackCompositionState.js",
      "github:capaj/systemjs-hot-reloader@0.5.8/default-listener.js",
      "github:capaj/systemjs-hot-reloader@0.5.8.json",
      "github:capaj/systemjs-hot-reloader@0.5.8/hot-reloader.js",
      "npm:debug@2.2.0/browser.js",
      "npm:debug@2.2.0.json",
      "npm:debug@2.2.0/debug.js",
      "npm:ms@0.7.1/index.js",
      "npm:ms@0.7.1.json",
      "npm:weakee@1.0.0/weakee.js",
      "npm:weakee@1.0.0.json",
      "github:socketio/socket.io-client@1.4.6/socket.io.js",
      "github:socketio/socket.io-client@1.4.6.json",
      "styles/vertical-tab-support-for-angular-ui-bootstrap.css!github:systemjs/plugin-css@0.1.21/css.js",
      "styles/lexical.less!github:systemjs/plugin-css@0.1.21/css.js",
      "styles/site.less!github:systemjs/plugin-css@0.1.21/css.js"
    ]
  }
});