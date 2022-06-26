import pika
from datetime import datetime

def setupChannel(_connection, _queue):
    _channel = _connection.channel()
    
    _args = {
        'x-message-ttl': 40000
    }
    
    _queue_A = _queue + '_A'
    _channel.queue_declare(queue=_queue_A, durable=False, exclusive=False, arguments=_args)
    
    _queue_B = _queue + '_B'
    _channel.queue_declare(queue=_queue_B, durable=False, exclusive=False, arguments=_args)
    
    _queue_C = _queue + '_C'
    _channel.queue_declare(queue=_queue_C, durable=False, exclusive=False, arguments=_args)
    
    _channel.exchange_declare(exchange='envio_para_A_B_C', exchange_type='fanout')
    
    _channel.queue_bind(exchange='envio_para_A_B_C', queue=_queue_A)
    _channel.queue_bind(exchange='envio_para_A_B_C', queue=_queue_B)
    _channel.queue_bind(exchange='envio_para_A_B_C', queue=_queue_C)
    
    return _channel

def BuildPublisher(_channel, _produtor):
    for i in range(5):
        message = (datetime.now()).strftime("%Y-%m-%d %H:%M:%S")
        print_message = "{p1}: {p2}".format(p1=_produtor, p2=message)
        
        channel.basic_publish(exchange='envio_para_A_B_C',
                            routing_key='',
                            body=message)
        print(print_message)

connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = setupChannel(connection, 'mensagens')
BuildPublisher(channel, 'Produtor A')

connection.close()