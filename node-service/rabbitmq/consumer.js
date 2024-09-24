// consumer.js
require('dotenv').config(); // Load environment variables from .env file
const amqp = require("amqplib/callback_api");
const { ActionTypes } = require("../constants");
// const ActionType = require

async function startConsumer() {
    amqp.connect(`amqp://${process.env.RABBITMQ_HOST}`, (err, connection) => {
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
                    const message = JSON.parse(msg.content)
                    switch(message.ActionType){
                        case ActionTypes.Added:
                            console.log('add')
                            break;
                        default:
                            console.log('default')
                            break;
                    }
                    console.log(
                        `[Consumer] Received: `, message
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
