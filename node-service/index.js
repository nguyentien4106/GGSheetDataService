const cron = require("node-cron");
const db = require("./db/index.js");
const connectRabbitMQ = require("./rabbitmq");
const { startConsumer } = require("./rabbitmq/consumer.js");

function startAsync() {
    startConsumer();
}

function onExecute() {
    // Schedule the cron job to run every 10 seconds
    cron.schedule("*/10 * * * * *", () => {
        console.log("Running cron job every 10 seconds");
        executeCronJob();
    });
}
function exitHandler(options, exitCode) {
    if (options.cleanup) console.log("clean");
    if (exitCode || exitCode === 0) console.log(exitCode);
    if (options.exit) process.exit();
}

// do something when app is closing
process.on("exit", exitHandler.bind(null, { cleanup: true }));

// catches ctrl+c event
process.on("SIGINT", exitHandler.bind(null, { exit: true }));

async function executeCronJob() {
    try {
        const { rows } = await db.query('SELECT * FROM "Devices"');
        // const channel = await cnectRabbitMQ();
        console.log(rows);
        // for (const row of rows) {
        //   await addMessageToQueue(channel, JSON.stringify(row));
        // }
    } catch (error) {
        console.error("Error in cron job:", error);
    }
}

startAsync();
onExecute();
