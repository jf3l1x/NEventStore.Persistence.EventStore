//function getBucketState(state, ev) {
//    if (!state[ev.body.BucketId]) {
//        state[ev.body.BucketId] = {};
//    }
//    return state[ev.body.BucketId];
//}

//function getStreamState(state, ev) {
//    var bucket = getBucketState(state, ev);
//    if (!state[ev.body.BucketId]) {
//        state[ev.body.BucketId] = {};
//    }
//    if (!bucket[ev.body.StreamId]) {
//        bucket[ev.body.StreamId] ={
//            StreamRevision: 0,
//            SnapshotRevision: 0
//        };
//    }
//    return bucket[ev.body.StreamId]; 
//}

fromCategory('nes.streamstosnapshot.accumulator')
    .foreachStream()
    .when({
        "$init": function() {
            return {
                count:0
            };

        },
            eventCreated: function(state, ev) {
                //var streamState = getStreamState(state, ev);
                if (!state[ev.body.BucketId]) {
                    state[ev.body.BucketId] = {};
                }
                if (!state[ev.body.BucketId][ev.body.StreamId]) {
                    state[ev.body.BucketId][ev.body.StreamId] = {
                        StreamRevision: 0,
                        SnapshotRevision: 0
                    };
                }
                state.count += ev.body.StreamRevision;
                state[ev.body.BucketId][ev.body.StreamId].StreamRevision = ev.body.StreamRevision;
                emit("testes", "state", state);
                return state;
            },
            snapshotCreated: function(state, ev) {
                //var streamState = getStreamState(state, ev);
                if (!state[ev.body.BucketId]) {
                    state[ev.body.BucketId] = {};
                }
                if (!state[ev.body.BucketId][ev.body.StreamId]) {
                    state[ev.body.BucketId][ev.body.StreamId] = {
                        StreamRevision: 0,
                        SnapshotRevision: 0
                    };
                }
                state.count -= ev.body.StreamRevision;
                state[ev.body.BucketId][ev.body.StreamId].SnapshotRevision = ev.body.StreamRevision;
                emit("testes", "state", state);
                //if (streamState.StreamRevision - streamState.SnapshotRevision < {{MinimunSnapshotThreshold}}) {
                //    delete state[ev.body.BucketId][ev.body.StreamId];
                //}
                return state;

            }
        }
    );