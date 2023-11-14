# TradePulse

### description
This service is designed to save changes in the exchange cup and process this data. It automatically searches for dump or pump zones.


Tech: Kafka, Mongo DB, docker
Algorithms: binary search tree



### API
```
GET /api/orderbook/data
```
**desc**: respond asks \& bids data array for specific symbol
params:
- symbol - name of symbol like BTCUSDT, OPUSDT, etc
- fromTs - timestamp in milliseconds, it request data from this point
- count - specifies the maximum number of elements to be returned

```
GET /api/orderbook/anomaly-zones
```
**desc**: respond anomaly zones. It is zones where was pump or dump
params:
- symbol - name of symbol like BTCUSDT, OPUSDT, etc
- fromTs - timestamp in milliseconds, it request data from this point
- count - specifies the maximum number of elements to be returned
