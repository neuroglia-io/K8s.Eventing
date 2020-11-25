fromCategory('cloudevents')
    .when({
        $any: function (s, e) {
            linkTo('$cloudevent-source-' + e.data.source, e);
        }
    });