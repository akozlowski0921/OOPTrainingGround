// ❌ BAD: Incompatible logging libraries without adapter

class OldLogger {
  writeLog(level: string, message: string, timestamp: Date): void {
    console.log(`[${timestamp.toISOString()}] ${level}: ${message}`);
  }

  writeError(message: string, error: Error): void {
    console.error(`ERROR: ${message}`, error);
  }
}

class NewLogger {
  log(data: { level: string; msg: string; meta?: any }): void {
    console.log(JSON.stringify(data));
  }
}

// Problem: Application expects one interface but has different loggers
class Application {
  constructor(private logger: OldLogger) {}

  run(): void {
    this.logger.writeLog("INFO", "App started", new Date());
    // Cannot use NewLogger here!
  }
}
