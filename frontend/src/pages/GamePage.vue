<template>
  <q-page class='row justify-around items-center'>
    <q-card @click="send"
      fit
      style="width: 80%; height: 600px;"
    >
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
  import * as signalR from "@microsoft/signalr";

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5144/Game", {accessTokenFactory: () => "real access token goes here (maybe refresh before returning)"})
    .configureLogging(signalR.LogLevel.Information)
    .build();

  async function start() {
      try {
          await connection.start();
          console.log("SignalR Connected.");
      } catch (err) {
          console.log(err);
          setTimeout(start, 5000);
      }
  };

  connection.onclose(async () => {
      await start();
  });

  connection.on("receiveMethod", (msg) => console.log(msg))

  // Start the connection.
  start();

  function send()
  {
    connection.send("Echo", "message");
  }
</script>