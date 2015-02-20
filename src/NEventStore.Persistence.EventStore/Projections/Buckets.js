fromCategory('nesevents')
  .foreachStream()
  .whenAny(
      function (state, ev) {
          if (!state.buckets) {
              state.buckets = {};
          }
          if (ev.metadata) {
              var bucketId = "";
              for (var i = 0; i < ev.metadata.length; i++) {
                  if (ev.metadata[i].Key === "BucketId") {
                      bucketId = ev.metadata[i].Value;
                      break;
                  }
              }
              if (bucketId !== "") {
                  if (!state.buckets[bucketId]) {
                      state.buckets[bucketId] = true;
                      emit("nesbuckets", "bucketCreated", bucketId);
                  }
                  if (ev.sequenceNumber === 0) {
                      //first event on stream, lets record the stream created 
                      emit("nesstreams-" + bucketId, "streamCreated", ev.streamId);
                  }
              }

          }
          return state;
      }
  )
