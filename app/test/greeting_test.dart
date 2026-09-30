import 'package:flutter_test/flutter_test.dart';
import 'package:hello_app/greeting.dart';

void main() {
  test('returns greeting for a valid name', () {
    expect(buildGreeting('  Ada '), 'Hello, Ada!');
  });

  test('returns null for blank names', () {
    expect(buildGreeting(null), isNull);
    expect(buildGreeting('   '), isNull);
  });
}
