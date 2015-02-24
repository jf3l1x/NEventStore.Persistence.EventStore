fromCategory('nessnapshots')
    .foreachStream()
    .whenAny(
        function(state, ev) {
            if (ev.body.BucketId && ev.body.StreamId) {
                emit("nes.streamstosnapshot.accumulator-snapshots", "snapshotCreated", {
                    StreamId: ev.body.StreamId,
                    BucketId: ev.body.BucketId,
                    StreamRevision: ev.body.StreamRevision
                });
            }
            return state;

        }
    )