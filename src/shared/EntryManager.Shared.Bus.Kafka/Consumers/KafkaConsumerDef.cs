namespace EntryManager.Shared.Bus.Kafka;

internal record KafkaConsumerDef(Type MessageType, string Topic, string Group);