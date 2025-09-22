// Example heartbeat payload that devices should send to topic: device/{deviceId}/hb
// This is just for documentation - not part of the application

/*
JSON Payload Example:
{
  "ts": 1695398400,                    // Unix timestamp (seconds)
  "boot_id": "550e8400-e29b-41d4-a716-446655440000",  // Unique boot identifier
  "seq": 42,                           // Sequence number (increments with each heartbeat)
  "uptime_s": 3600,                    // Device uptime in seconds
  "cfg_hash": "abc123def456",          // Configuration hash (optional)
  "flags": ["ok", "throttled"],        // Status flags (optional)
  "extra": ["temp:25.5", "load:45"]    // Extra information (optional)
}

MQTT Topic: device/00000000-0000-0000-0000-000000000001/hb

The system will:
1. Parse the device ID from the MQTT topic
2. Deserialize the JSON payload into a HeartbeatDTO
3. Update the device state in the database
4. Set connectivity status to Online
5. Update last heartbeat timestamp
6. Monitor for stale devices in the background

API Endpoints:
- POST /api/device/{id}/heartbeat - Manual heartbeat submission for testing
- GET /api/device/stale?minutesThreshold=5 - Get devices with stale heartbeats

Background Services:
- MqttServerService - Handles MQTT message routing
- DeviceHealthMonitorService - Monitors device health and marks stale devices as offline
*/