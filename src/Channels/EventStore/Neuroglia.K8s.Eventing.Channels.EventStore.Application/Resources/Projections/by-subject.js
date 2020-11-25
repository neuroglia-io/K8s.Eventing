fromCategory('cloudevents')
    .when({
        $any: function (s, e) {
            linkTo('$cloudevent-subject-' + e.data.subject, e);
        }
    });
    