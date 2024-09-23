// consumer.js
const amqp = require("amqplib/callback_api");

async function startConsumer() {
    amqp.connect("amqp://localhost", (err, connection) => {
        if (err) {
            console.log(err);
            throw err;
        }

        // Create a channel
        connection.createChannel((err, channel) => {
            if (err) {
                throw err;
            }

            const queue = "DataService.DeviceEventQueue";

            // Assert a queue into existence
            channel.assertQueue(queue, {
                durable: true, // Messages will not be saved if RabbitMQ crashes
            });

            console.log(
                `[Consumer] Waiting for messages in ${queue}. To exit press CTRL+C`
            );

            // Consume messages from the queue
            channel.consume(queue, (msg) => {
                if (msg !== null) {
                    const json = JSON.parse(msg.content)
                    console.log(
                        `[Consumer] Received: `, json
                    );
                    channel.ack(msg); // Acknowledge that the message has been processed
                }
            });
        });
    });
}

module.exports = {
    startConsumer: startConsumer,
};
// startConsumer();
