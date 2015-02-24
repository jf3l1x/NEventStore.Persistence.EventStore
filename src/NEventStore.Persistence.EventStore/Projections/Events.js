fromCategory('nesevents')
  .foreachStream()
  .when({
      "$init": function () {
          return {
              buckets: {}
          };
      },
      "$any": function(state, ev) {
            if (ev.metadata) {
                var bucketId = ev.metadata.BucketId;
                var streamId = ev.metadata.StreamId;
                if (bucketId ) {
                    if (!state.buckets[bucketId]) {
                        state.buckets[bucketId] = true;
                        emit("nesbuckets", "bucketCreated", bucketId);
                    }
                    if (ev.sequenceNumber === 0 && streamId) {
                        //first event on stream, lets record the stream created 
                        emit("nesstreams-" + bucketId, "streamCreated",
                        {
                            StreamId: streamId,
                            BucketId: bucketId
                        });
                    }
                    if (streamId && ev.metadata.StreamRevision% {{MinimunSnapshotThreshold}}===0) {
                        emit("nes.streamstosnapshot.accumulator-events", "eventCreated", {
                            StreamId: streamId,
                            BucketId: bucketId,
                            StreamRevision: ev.metadata.StreamRevision
                    });
                    }


                }

            }
            return state;
        }
    }
     
  )
