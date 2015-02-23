fromCategory('nesevents')
  .foreachStream()
  .whenAny(
      function (state, ev) {
          if (!state.buckets) {
              state.buckets = {};
          }
          if (ev.metadata) {
              var bucketId = "";
              var streamId = "";
              for (var i = 0; i < ev.metadata.length; i++) {
                  if (ev.metadata[i].Key === "BucketId") {
                      bucketId = ev.metadata[i].Value;
                  }
                  if (ev.metadata[i].key == "StreamId") {
                      streamId = bucketId = ev.metadata[i].Value;
                  }
              }
              if (bucketId !== "") {
                  if (!state.buckets[bucketId]) {
                      state.buckets[bucketId] = true;
                      emit("nesbuckets", "bucketCreated", bucketId);
                  }
                  if (ev.sequenceNumber === 0) {
                      //first event on stream, lets record the stream created 
                      emit("nesstreams-" + bucketId, "streamCreated",
                          {
                            StreamId: streamId,
                            BucketId: bucketId
                          });
                  }
              }

          }
          return state;
      }
  )
