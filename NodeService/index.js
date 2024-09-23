const cron = require('node-cron');
const db = require('./db/index.js');
const connectRabbitMQ = require('./rabbitmq');
const { startConsumer } = require("./rabbitmq/consumer.js")


async function executeCronJob() {
  try {
    const { rows } = await db.query('SELECT * FROM "Devices"');
    // const channel = await cnectRabbitMQ();
    console.log(rows)
    // for (const row of rows) {
    //   await addMessageToQueue(channel, JSON.stringify(row));
    // }
  } catch (error) {
    console.error('Error in cron job:', error);
  }
}
startConsumer();
// Schedule the cron job to run every 10 seconds
cron.schedule('*/10 * * * * *', () => {
  console.log('Running cron job every 10 seconds');
  executeCronJob();
});