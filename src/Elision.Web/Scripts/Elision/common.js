System.register("modules/moduleLoader",[],function(a){var b;return{setters:[],execute:function(){"use strict";b={init:function(){var a=Array.prototype.slice.call(document.querySelectorAll("[data-module]"));a.forEach(this.loadModule)},loadModule:function(a){var b=a.getAttribute("data-module");require([b],function(a){})}},a("default",b)}}}),System.register("app",["modules/moduleLoader"],function(a){var b,c;return{setters:[function(a){b=a["default"]}],execute:function(){"use strict";c=function(){b.init()},a("default",c)}}});