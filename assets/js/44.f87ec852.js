(window.webpackJsonp=window.webpackJsonp||[]).push([[44],{415:function(e,o,t){"use strict";t.r(o);var n=t(0),s=Object(n.a)({},(function(){var e=this,o=e.$createElement,t=e._self._c||o;return t("ContentSlotsDistributor",{attrs:{"slot-key":e.$parent.slotKey}},[t("h1",{attrs:{id:"hub-endpoints"}},[t("a",{staticClass:"header-anchor",attrs:{href:"#hub-endpoints"}},[e._v("#")]),e._v(" Hub Endpoints")]),e._v(" "),t("p",[e._v("The core of communication contracts between the client and server are hubs. Depending on your application and complexity you might have a few hubs as a separation of concern for your application. The backplanes work through 5 types of events "),t("strong",[e._v("per hub")]),e._v(".")]),e._v(" "),t("p",[e._v("So this translated well into MassTransit Events:")]),e._v(" "),t("ul",[t("li",[t("code",[e._v("All<THub>")]),e._v(" - Invokes the method (with args) for each connection on the specified hub")]),e._v(" "),t("li",[t("code",[e._v("Connection<THub>")]),e._v(" - Invokes the method (with args) for the specific connection")]),e._v(" "),t("li",[t("code",[e._v("Group<THub>")]),e._v(" - Invokes the method (with args) for all connections belonging to the specified group")]),e._v(" "),t("li",[t("code",[e._v("GroupManagement<THub>")]),e._v(" - Adds or removes a connection to the group (on a remote server)")]),e._v(" "),t("li",[t("code",[e._v("User<THub>")]),e._v(" - Invokes the method (with args) for all connections belonging to the specific user id")])]),e._v(" "),t("p",[e._v("So each of these Messages has a corresponding consumer, and it will get a singleton "),t("code",[e._v("HubLifetimeManager<THub>")]),e._v(" through DI to perform the specific task.")]),e._v(" "),t("p",[e._v("MassTransit's helper extension method will create an endpoint per consumer per hub, which follows the typical recommendation of one consumer per endpoint. Because of this, the number of endpoints can grow quickly if you have many hubs. It's best to also read some "),t("a",{attrs:{href:"https://docs.microsoft.com/en-us/aspnet/signalr/overview/performance/scaleout-in-signalr#limitations",target:"_blank",rel:"noopener noreferrer"}},[e._v("SignalR Limitations"),t("OutboundLink")],1),e._v(", to understand what can become potential bottlenecks with SignalR and your backplane. SignalR recommends re-thinking your strategy for very high throughput, real-time applications (video games).")])])}),[],!1,null,null,null);o.default=s.exports}}]);