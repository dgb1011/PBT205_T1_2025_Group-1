#include <iostream>
#include <string>
#include <vector>
#include <map>
#include <algorithm>

struct Order {
    std::string traderID;
    std::string side; // "BUY" or "SELL"
    int quantity;
    double price;
};

struct Trade {
    std::string buyerID;
    std::string sellerID;
    int quantity;
    double price;
};

class Exchange {
private:
    std::vector<Order> buyOrders;
    std::vector<Order> sellOrders;

public:
    void processOrder(const Order& order) {
        if (order.side == "BUY") {
            matchOrder(order, sellOrders, buyOrders);
        } else if (order.side == "SELL") {
            matchOrder(order, buyOrders, sellOrders);
        }
    }

    void matchOrder(const Order& newOrder, std::vector<Order>& oppositeOrders, std::vector<Order>& sameSideOrders) {
        auto it = std::find_if(oppositeOrders.begin(), oppositeOrders.end(), [&](const Order& existingOrder) {
            return (newOrder.side == "BUY" && newOrder.price >= existingOrder.price) ||
                   (newOrder.side == "SELL" && newOrder.price <= existingOrder.price);
        });

        if (it != oppositeOrders.end()) {
            Trade trade = {newOrder.traderID, it->traderID, newOrder.quantity, it->price};
            publishTrade(trade);
            oppositeOrders.erase(it); // Remove matched order
        } else {
            sameSideOrders.push_back(newOrder); // Add to order book if no match
        }
    }

    void publishTrade(const Trade& trade) {
        std::cout << "Trade Executed: Buyer: " << trade.buyerID
                  << ", Seller: " << trade.sellerID
                  << ", Quantity: " << trade.quantity
                  << ", Price: $" << trade.price << "\n";
    }
};

// Trader Applocaction

#include <iostream>
#include <string>

struct Order {
    std::string traderID;
    std::string side; // "BUY" or "SELL"
    int quantity;
    double price;
};

void sendOrder(const std::string& traderID, const std::string& side, int quantity, double price) {
    // Simulate sending an order to the Exchange
    Order order = {traderID, side, quantity, price};
    std::cout << "Order Sent: Trader: " << order.traderID
              << ", Side: " << order.side
              << ", Quantity: " << order.quantity
              << ", Price: $" << order.price << "\n";
}

int main(int argc, char* argv[]) {
    if (argc != 5) {
        std::cerr << "Usage: <Trader ID> <Side> <Quantity> <Price>\n";
        return 1;
    }

    std::string traderID = argv[1];
    std::string side = argv[2];
    int quantity = std::stoi(argv[3]);
    double price = std::stod(argv[4]);

    sendOrder(traderID, side, quantity, price);
    return 0;
}
