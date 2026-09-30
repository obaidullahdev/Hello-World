import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:hello_app/main.dart';

void main() {
  testWidgets('greets the entered name', (tester) async {
    await tester.pumpWidget(const HelloApp());

    await tester.enterText(find.byType(TextField), 'Ada');
    await tester.tap(find.text('Greet'));
    await tester.pump();

    expect(find.text('Hello, Ada!'), findsOneWidget);
  });

  testWidgets('shows an error for a blank name', (tester) async {
    await tester.pumpWidget(const HelloApp());

    await tester.tap(find.text('Greet'));
    await tester.pump();

    expect(find.text('Name must not be blank.'), findsOneWidget);
  });
}
