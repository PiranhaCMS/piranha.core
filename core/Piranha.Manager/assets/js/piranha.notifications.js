//
// Copyright (c) 2018 Filip Jansson
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
// 
// http://github.com/piranhacms/piranha.core
// 

// Model/template
//
// Simple notification
//  {
//      class: "alert-success",
//      icon: "fas fa-check",
//      body: "Text"
//  }
//
// Notification with buttons
//  {
//      class: "alert-success",
//      body: "HTML",
//      buttons: [
//          {
//              text: "Dismiss",
//              class: "btn-success"
//          },
//          {
//              text: "Create",
//              handler: function () {
//                  console.log("Creating...");
//                  var notis = piranha.notifications.findNotification(this);
//                  piranha.notifications.dismiss(notis);
//              }
//          }
//      ]
//  }

if (typeof(piranha)  == 'undefined')
    piranha = {};

piranha.notifications = new function() {
    'use strict';

    var self = this;

    /**
     * Notification container
     */
    self.container = document.getElementById("notifications-container");

    /**
     * Shortcut for creating a success notification
     * @param {string} msg
     * @param {string} icon
     */
    self.createSuccess = function (msg, icon) {
        var n = self.create({
            class: "alert-success",
            icon: icon || "fas fa-check",
            body: msg
        });

        self.add(n);
    }

    /**
     * Shortcut for creating a info notification
     * @param {string} msg
     * @param {string} icon
     */
    self.createInfo = function (msg, icon) {
        var n = self.create({
            class: "alert-info",
            icon: icon || "fas fa-info-circle",
            body: msg
        });

        self.add(n);
    }

    /**
     * Shortcut for creating a warning notification
     * @param {string} msg
     * @param {string} icon
     */
    self.createWarning = function (msg, icon) {
        var n = self.create({
            class: "alert-warning",
            icon: icon || "fas fa-exclamation-triangle",
            body: msg
        });

        self.add(n);
    }

    /**
     * Shortcut for creating a danger notification
     * @param {string} msg
     * @param {string} icon
     */
    self.createDanger = function (msg, icon) {
        var n = self.create({
            class: "alert-danger",
            icon: icon || "fas fa-exclamation-triangle",
            body: msg
        });

        self.add(n);
    }

    /**
     * Create a new notification
     * @param {object} model
     */
    self.create = function (model) {

        // Create the element with basic styling
        var notification = document.createElement("div");
        notification.className = "alert notification-alert";
        notification.classList.add(model.class || "alert-info");

        // If we have an icon,
        // we use a more simpler notification layout
        if (model.icon) {
            var p = document.createElement("p");
            var i = document.createElement("i");

            i.className = model.icon;
            p.appendChild(i);

            var text = document.createTextNode(model.body);
            p.appendChild(text);

            notification.appendChild(p);
        } else {
            notification.innerHTML = model.body;
        }

        // Check to see if any buttons exist
        if (model.buttons && model.buttons.length > 0) {
            notification.appendChild(document.createElement("hr"));

            // Create a element that holds the buttons
            var group = document.createElement("div");
            group.classList.add("text-right");

            model.buttons.forEach(function (button) {
                var btn = document.createElement("button");
                
                // Default classes
                btn.classList.add("btn");
                btn.classList.add("btn-sm");

                btn.innerText = button.text;
                
                // Use custom class or default
                if (button.class) {
                    btn.classList.add(button.class);
                } else {
                    btn.classList.add("btn-default");
                }

                // Attacht handler, or if non exist
                // we add a defult dismiss handler
                if (button.handler) {
                    btn.addEventListener("click", button.handler);
                } else {
                    btn.addEventListener("click", function () {
                        var n = piranha.notifications.findNotification(this);
                        piranha.notifications.dismiss(n);
                    });
                }

                group.appendChild(btn);
            });

            notification.appendChild(group);
        };

        return notification;
    }

    /**
     * Add the notification to the DOM
     * @param {element} notification
     * @param {number} countDown time (ms) before dismiss. 
     */
    self.add = function (notification, countDown) {

        if (typeof countDown === 'undefined')
            countDown = 3500;

        // Remove dismiss class if any.
        // Can be on the element if it already been added once.
        notification.classList.remove("notification-dismiss");

        self.container.appendChild(notification);

        if (countDown > 0) {
            setTimeout(function () {
                self.dismiss(notification);
            }, countDown)
        }
    };

    /**
     * Find the notification element
     * from a child element
     * @param {element} child
     */
    self.findNotification = function (child) {
        if (child) {
            while ((child = child.parentElement) && !child.classList.contains("notification-alert"));
            return child;
        }
        return;
    }

    /**
     * Run the dismiss sequence
     * and remove the notification
     * @param {element} notification
     */
    self.dismiss = function (notification) {
        notification.classList.add("notification-dismiss");
        setTimeout(function () {
            notification.remove();
        }, 400);
    }
};