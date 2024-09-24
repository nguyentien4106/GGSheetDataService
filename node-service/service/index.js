const ZKTeco = require("zkteco");

export const test = async () => {
  try {
    // Define the IP address of the device.
    const deviceIp = "192.168.1.201";

    //  Ips
    // List of devices with their respective IP addresses and ports.
    const devices = [{ deviceIp: "192.168.1.201", devicePort: "4370" }];
    let zkInstance = new ZKTeco(devices);

    // Connect all devices
    await zkInstance.connectAll();

    // Retrieve users based on device IP addresses in the machine.
    const users = await zkInstance.getUsers(deviceIp);

    // Retrieve all devices currently connected.
    const getAllConnectedIps = await zkInstance.getAllConnectedIps();

    await zkInstance.disconnect()
    return 'success'
  } catch (e) {
    console.log(e);
    if (e.code === "EADDRINUSE") {
    }

    return 'fail'
  }
};