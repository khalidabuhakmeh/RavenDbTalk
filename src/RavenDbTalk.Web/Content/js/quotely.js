var Quotely = Quotely || {};

Quotely.LiveViewModel = function () {
    var self = this;
    var hub = $.connection.liveHub;

    self.name = ko.observable('');
    self.text = ko.observable('');
    self.thoughts = ko.observableArray();

    self.any = ko.computed(function () {
        return self.thoughts().length > 0;
    });

    self.empty = ko.computed(function () {
        return !self.any();
    });

    self.create = function () {
        var thought = {
            text: self.text(),
            name: self.name()
        };
        hub.server.submit(thought).done(function() {
            self.text('');
            self.name('');
        });
    };

    self.add = function (thought) {
        self.thoughts.unshift(new Quotely.ThoughtViewModel(thought));
    };

    self.show = function (elem, vm) {
        $(elem).hide().show('blind');
    };

    // SignalR methods
    hub.client.addThought = function (thought) {
        self.add(thought);
    };

    hub.client.addError = function(message) {
        console.log(message);
    };

    // change to true to see more info from SignalR
    $.connection.hub.logging = true;
    $.connection.hub.start();
};

Quotely.ThoughtViewModel = function (json) {
    var self = this;
    json = ko.toJS(json);

    self.name = json.name;
    self.text = json.text;
};