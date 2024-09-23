// const amqp = require('amqplib');

// async function connect() {
//   try {
//     const connection = await amqp.connect('amqp://localhost');
//     const channel = await connection.createChannel();
//     await channel.assertQueue('task_queue', { durable: true });
//     return channel;
//   } catch (error) {
//     console.error('Error connecting to RabbitMQ:', error);
//   }
// }

// module.exports = connect;