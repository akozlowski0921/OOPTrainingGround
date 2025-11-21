// ✅ GOOD: Adapter pattern for unified logging interface

interface Logger {
  info(message: string, meta?: any): void;
  error(message: string, error?: Error): void;
}

class OldLoggerLib {
  writeLog(level: string, message: string, timestamp: Date): void {
    console.log(`[${timestamp.toISOString()}] ${level}: ${message}`);
  }

  writeError(message: string, error: Error): void {
    console.error(`ERROR: ${message}`, error);
  }
}

class NewLoggerLib {
  log(data: { level: string; msg: string; meta?: any }): void {
    console.log(JSON.stringify(data));
  }
}

// ✅ Adapter for old logger
class OldLoggerAdapter implements Logger {
  constructor(private oldLogger: OldLoggerLib) {}

  info(message: string, meta?: any): void {
    this.oldLogger.writeLog("INFO", message, new Date());
  }

  error(message: string, error?: Error): void {
    this.oldLogger.writeError(message, error || new Error(message));
  }
}

// ✅ Adapter for new logger
class NewLoggerAdapter implements Logger {
  constructor(private newLogger: NewLoggerLib) {}

  info(message: string, meta?: any): void {
    this.newLogger.log({ level: "info", msg: message, meta });
  }

  error(message: string, error?: Error): void {
    this.newLogger.log({ level: "error", msg: message, meta: { error } });
  }
}

// ✅ Application works with any logger
class Application {
  constructor(private logger: Logger) {}

  run(): void {
    this.logger.info("App started");
    this.logger.error("Something went wrong", new Error("Test error"));
  }
}

const app1 = new Application(new OldLoggerAdapter(new OldLoggerLib()));
const app2 = new Application(new NewLoggerAdapter(new NewLoggerLib()));
